#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

SOLUTION_NAME="Finova"
ROOT_DIR="$(pwd)"
PRES_PROJ="${ROOT_DIR}/src/${SOLUTION_NAME}.Presentation.WinForms/${SOLUTION_NAME}.Presentation.WinForms.csproj"

# Put MSBuild temp on D: to avoid C: disk pressure
TEMP_ROOT="/d/Temp"
MSBUILD_TEMP="${TEMP_ROOT}/MSBuildTemp"
mkdir -p "${MSBUILD_TEMP}"

export TEMP="$(cygpath -w "${MSBUILD_TEMP}")"
export TMP="$(cygpath -w "${MSBUILD_TEMP}")"
export DOTNET_ENVIRONMENT="${DOTNET_ENVIRONMENT:-Development}"

echo "TEMP=${TEMP}"
echo "DOTNET_ENVIRONMENT=${DOTNET_ENVIRONMENT}"
dotnet run --project "${PRES_PROJ}" -c Debug
