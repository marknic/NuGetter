tf checkout "D:\_TfsProjects\BuildActivities\Custom Activity Storage\TfsBuild.NuGetter.Activities.dll"

del /F "D:\_TfsProjects\BuildActivities\Custom Activity Storage\TfsBuild.NuGetter.Activities.dll"

copy /Y /B "D:\_TfsProjects\BuildActivities\NuGetter\Dev\V1.1.1.0\Source\TfsBuild.NuGetter.Activities\bin\Debug\TfsBuild.NuGetter.Activities.dll" "D:\_TfsProjects\BuildActivities\Custom Activity Storage\TfsBuild.NuGetter.Activities.dll"

tf checkin /noprompt /comment:"Updating Activities" "D:\_TfsProjects\BuildActivities\Custom Activity Storage\TfsBuild.NuGetter.Activities.dll"
