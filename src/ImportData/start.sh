#!/bin/bash
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR/ImportDataUI" || exit 1
dotnet ./ImportUtilServer.dll