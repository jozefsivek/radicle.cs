Deploy checklist
================

 1. sync with upstream: `$ git co master && git fetch --all -p && git pull`
 2. list tags: `$ git tag`
 3. check if [changelog](CHANGELOG.md) is up to date
 4. check the [Directory.Build.props](Directory.Build.props) has up to date version
 5. test deply and pack with [ci-test](ci-test)
 6. attach tag to HEAD: `$ git tag -a vX.Y.Z[-alpha.X|-beta.X]`
 7. check proper tag label in `$ git log`
 8. sync tags upstream: `$ git push origin --tags`
 9. check the action succeeded
10. check nuget.org state
11. wrap up release if needed
