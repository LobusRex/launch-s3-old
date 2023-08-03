# Sizes come from the Icon Scaling table at
# https://learn.microsoft.com/en-us/windows/apps/design/style/iconography/app-icon-construction
$sizes = 16, 20, 24, 30, 32, 36, 40, 48, 60, 64, 72, 80, 96, 256

New-Item -ItemType "directory" -Path .\temp -Force

# Create Png images.
Foreach ($size in $sizes)
{
	inkscape playbbob.svg --export-filename=temp\$size.png -w $size -h $size
}

# Create an Ico file from the images.
$images = @()
Foreach ($size in $sizes)
{
	$images += "temp\\{0}{1}" -f $size,".png"
}
convert $images playbbob.ico

Remove-Item -Path .\temp -Recurse
