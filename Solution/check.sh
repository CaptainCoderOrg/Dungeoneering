#!/bin/bash
dotnet format style --verify-no-changes --verbosity diagnostic
dotnet build
dotnet test -p:CollectCoverage=true -p:CoverletOutputFormat=cobertura
reportgenerator -reports:Tests/coverage.cobertura.xml  -targetdir:html-coverage-report/