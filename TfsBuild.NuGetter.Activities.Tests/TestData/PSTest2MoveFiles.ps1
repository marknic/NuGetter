# $tfsDropFolder is automatically provided via the Workflow

$DestinationFolder = "$tfsDropFolder\\PackageFolder"
New-Item $DestinationFolder -ItemType Directory

$sourceFile = "$tfsDropFolder\\DummyTestFile01.dat"
Move-Item -Path $sourceFile -Destination $DestinationFolder
