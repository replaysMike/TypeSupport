Write-Host "Submitting Codacy Coverage (commit $env:APPVEYOR_REPO_COMMIT)" -ForegroundColor green

Start-FileDownload 'https://github.com/codacy/codacy-coverage-reporter/releases/download/4.0.5/codacy-coverage-reporter-4.0.5-assembly.jar'

Get-ChildItem ".\TypeSupport\TypeSupport.Tests\" -recurse | where {$_.name -eq "coverage.cobertura.xml"} | % { java -jar codacy-coverage-reporter-4.0.5-assembly.jar report -l CSharp -r $_.FullName --partial --project-token "$env:CODACY_TOKEN" --commit-uuid "$env:APPVEYOR_REPO_COMMIT" | Select-String -Pattern "INFO" | Select-String -Pattern "-INFO" -NotMatch }

java -jar codacy-coverage-reporter-4.0.5-assembly.jar final --project-token "$env:CODACY_TOKEN" --commit-uuid "$env:APPVEYOR_REPO_COMMIT" | Select-String -Pattern "INFO" | Select-String -Pattern "-INFO" -NotMatch