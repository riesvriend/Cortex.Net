git -c http.extraheader="AUTHORIZATION: basic $Env:token" submodule add -f --name Cortex.Net.Docs "../Cortex.Net.Docs.git" "docs/_site"
cd docs
dir
docfx docfx.json
cd _site
git add .
git commit -m "Autogenerated Documentation."
git -c http.extraheader="AUTHORIZATION: basic $Env:token" push