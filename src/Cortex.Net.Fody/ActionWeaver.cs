﻿// <copyright file="ActionWeaver.cs" company="Jan-Willem Spuij">
// Copyright 2019 Jan-Willem Spuij
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
// the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace Cortex.Net.Fody
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Cortex.Net.Api;
    using Cortex.Net.Fody.Properties;
    using Mono.Cecil;

    /// <summary>
    /// Weaves methods decorated with an <see cref="ActionAttribute"/>.
    /// </summary>
    internal class ActionWeaver
    {
        /// <summary>
        /// The prefix for an inner method that is the target of an action.
        /// </summary>
        private const string InnerMethodPrefix = "Cortex_Net_Fvclsnf97SxcMxlkizajkz_";

        /// <summary>
        /// The prefix for an inner field that contains the action.
        /// </summary>
        private const string InnerActionFieldPrefix = "cortex_Net_Dls82uidkJs37dlaksjid_";

        /// <summary>
        /// A reference to the parent Cortex.Net weaver.
        /// </summary>
        private readonly CortexWeaver cortexWeaver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionWeaver"/> class.
        /// </summary>
        /// <param name="cortexWeaver">A reference to the Parent Cortex.Net weaver.</param>
        public ActionWeaver(CortexWeaver cortexWeaver)
        {
            this.cortexWeaver = cortexWeaver ?? throw new ArgumentNullException(nameof(cortexWeaver));
        }

        /// <summary>
        /// Executes this action weaver.
        /// </summary>
        internal void Execute()
        {
            var decoratedMethods = from t in this.cortexWeaver.ModuleDefinition.GetTypes()
                             from m in t.Methods
                             where
                                t != null &&
                                t.IsClass &&
                                t.BaseType != null &&
                                m != null &&
                                m.CustomAttributes != null &&
                                m.CustomAttributes.Any(x => x.AttributeType.FullName == typeof(ActionAttribute).FullName)
                             select m;

            foreach (var method in decoratedMethods.ToList())
            {
                this.WeaveMethod(method);
            }
        }

        /// <summary>
        /// Duplicates a method with with the same body as the source method
        /// and adds it to the class to the class.
        /// </summary>
        /// <param name="sourceMethod">The source method.</param>
        private static void DuplicateAsInnerMethod(MethodDefinition sourceMethod)
        {
            var innerDefinition = new MethodDefinition($"{InnerMethodPrefix}{sourceMethod.Name}", (sourceMethod.Attributes & ~MethodAttributes.Public) | MethodAttributes.Private, sourceMethod.ReturnType);

            innerDefinition.Body = sourceMethod.Body;
            innerDefinition.IsStatic = sourceMethod.IsStatic;

            foreach (var parameter in sourceMethod.Parameters)
            {
                innerDefinition.Parameters.Add(parameter);
            }

            sourceMethod.DeclaringType.Methods.Add(innerDefinition);
        }

        /// <summary>
        /// Weaves a method that was Decorated with the <see cref="ActionAttribute"/>.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        private void WeaveMethod(MethodDefinition methodDefinition)
        {
            var moduleDefinition = methodDefinition.Module;
            var declaringType = methodDefinition.DeclaringType;

            // Duplicate the action method as private inner method.
            DuplicateAsInnerMethod(methodDefinition);

            var fieldAttributes = FieldAttributes.Private;
            if (methodDefinition.IsStatic)
            {
                fieldAttributes |= FieldAttributes.Static;
            }

            // convert the method definition to a corresponding Action<> delegate.
            var actionType = this.GetActionType(methodDefinition);

            // add the delegate as field to the class.
            var fieldDefinition = new FieldDefinition($"{InnerActionFieldPrefix}{methodDefinition.Name}_Action", fieldAttributes, actionType);
            declaringType.Fields.Add(fieldDefinition);

            var iObservableObjectInterfaceType = moduleDefinition.ImportReference(typeof(IObservableObject));
            var iObservableObjectinterfaceDefinition = new InterfaceImplementation(iObservableObjectInterfaceType);

            // If this object does not implement IObservableObject, add it, plus a default implementation.
            if (!declaringType.Interfaces.Contains(iObservableObjectinterfaceDefinition))
            {
                var getOverride = iObservableObjectInterfaceType.Resolve().Methods.Single(x => x.Name.Contains($"get_SharedState"));
                var setOverride = iObservableObjectInterfaceType.Resolve().Methods.Single(x => x.Name.Contains($"set_SharedState"));

                declaringType.Interfaces.Add(iObservableObjectinterfaceDefinition);

                var fieldTypeReference = moduleDefinition.ImportReference(typeof(ISharedState));

                // add backing field for shared state to the class
                var backingField = declaringType.CreateBackingField(fieldTypeReference, "Cortex.Net.Api.IObservableObject.SharedState");

                var methodAttributes = MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;

                // add getter
                var getter = declaringType.CreateDefaultGetter(backingField, "Cortex.Net.Api.IObservableObject.SharedState", methodAttributes);
                getter.Overrides.Add(moduleDefinition.ImportReference(getOverride));

                // add getter
                var setter = declaringType.CreateDefaultSetter(backingField, "Cortex.Net.Api.IObservableObject.SharedState", methodAttributes, null);
                setter.Overrides.Add(moduleDefinition.ImportReference(setOverride));

                // add property
                declaringType.CreateProperty("Cortex.Net.Api.IObservableObject.SharedState", getter, setter);
            }
        }

        /// <summary>
        /// Gets the Action type for the private field that is added to the class for the private method.
        /// </summary>
        /// <param name="methodDefinition">The method definition for the action.</param>
        /// <returns>A type reference.</returns>
        private TypeReference GetActionType(MethodDefinition methodDefinition)
        {
            var moduleDefinition = this.cortexWeaver.ModuleDefinition;

            if (methodDefinition.Parameters == null || !methodDefinition.Parameters.Any())
            {
                return moduleDefinition.ImportReference(typeof(Action));
            }

            TypeReference genericActionType;

            switch (methodDefinition.Parameters.Count)
            {
                case 1:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<>));
                    break;
                case 2:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,>));
                    break;
                case 3:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,>));
                    break;
                case 4:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,>));
                    break;
                case 5:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,>));
                    break;
                case 6:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,>));
                    break;
                case 7:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,>));
                    break;
                case 8:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,>));
                    break;
                case 9:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,>));
                    break;
                case 10:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,,>));
                    break;
                case 11:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,,,>));
                    break;
                case 12:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,,,,>));
                    break;
                case 13:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,,,,,>));
                    break;
                case 14:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,,,,,,>));
                    break;
                case 15:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,,,,,,,>));
                    break;
                case 16:
                    genericActionType = moduleDefinition.ImportReference(typeof(Action<,,,,,,,,,,,,,,,>));
                    break;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.MoreThan16Parameters, methodDefinition.Name));
            }

            var instance = new GenericInstanceType(genericActionType);

            foreach (var parameter in methodDefinition.Parameters)
            {
                instance.GenericArguments.Add(parameter.ParameterType);
            }

            return instance;
        }
    }
}
