#!/bin/sh
cd "${0%/*}" || exit                                # Run from this directory
. ${WM_PROJECT_DIR:?}/bin/tools/RunFunctions        # Tutorial run functions
#------------------------------------------------------------------------------

runApplication blockMesh
runApplication transformPoints -rollPitchYaw '(90 0 0)'
runApplication $(getApplication)
simpleFoam -postProcess -latestTime -func yPlus
