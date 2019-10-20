﻿// <copyright file="DerivationExtensions.cs" company="Michel Weststrate, Jan-Willem Spuij">
// Copyright 2019 Michel Weststrate, Jan-Willem Spuij
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

namespace Cortex.Net.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Extension methods for <see cref="IDerivation"/> interface implementations.
    /// </summary>
    public static class DerivationExtensions
    {
        /// <summary>
        /// Checks whether the <see cref="IDerivation"/>instance should recompute itself.
        /// </summary>
        /// <param name="derivation">The derivation.</param>
        /// <returns>True when it needs to recompute, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">When any of the arguments is null.</exception>
        /// <remarks>Might throw any other exception that a getter for <see cref="IObservable"/> will thow.</remarks>
        public static bool ShouldCompute(this IDerivation derivation)
        {
            if (derivation is null)
            {
                throw new ArgumentNullException(nameof(derivation));
            }

            switch (derivation.DependenciesState)
            {
                case DerivationState.UpToDate:
                    return false;
                case DerivationState.NotTracking:
                case DerivationState.Stale:
                    return true;
                case DerivationState.PossiblyStale:
                        // no need for those computeds to be reported, they will be picked up in trackDerivedFunction.
                    var previousDerivation = derivation.SharedState.StartUntracked();

                    foreach (var observable in derivation.Observing)
                    {
                        if (observable.IsDerivation())
                        {
                            if (observable.SharedState.Configuration.DisableErrorBoundaries)
                            {
                                observable.Get();
                            }
                        }
                        else
                        {
                            try
                            {
                                observable.Get();
                            }
#pragma warning disable CA1031 // Do not catch general exception types
                            catch
#pragma warning restore CA1031 // Do not catch general exception types
                            {
                                // we are not interested in the value *or* exception at this moment, but if there is one, notify all
                                derivation.SharedState.EndUntracked(previousDerivation);
                                return true;
                            }
                        }
                    }

                    derivation.ChangeLowestObserverStateOnObservablesToUpToDate();
                    derivation.SharedState.EndUntracked(previousDerivation);
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Changes the <see cref="IObservable.LowestObserverState"/> ona ll <see cref="IDerivation.Observing"/> to <see cref="DerivationState.UpToDate"/>
        /// when this derivation changes to <see cref="DerivationState.UpToDate"/>.
        /// </summary>
        /// <param name="derivation">The derivation to use.</param>
        private static void ChangeLowestObserverStateOnObservablesToUpToDate(this IDerivation derivation)
        {
            if (derivation.DependenciesState == DerivationState.UpToDate)
            {
                return;
            }

            derivation.DependenciesState = DerivationState.UpToDate;
            foreach (var observable in derivation.Observing)
            {
                observable.LowestObserverState = DerivationState.UpToDate;
            }
        }
    }
}
