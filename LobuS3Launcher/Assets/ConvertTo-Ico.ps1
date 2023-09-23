param(
	# The path to the Svg to convert.
	$inputSvg = '.\playbbob.svg',

	# The path to the new Ico file.
	$outputIco  = '.\playbbob.ico',

	# A place to temporarily store images in.
	$temp = '.\temp'
)

# Sizes come from the Icon Scaling table at
# https://learn.microsoft.com/en-us/windows/apps/design/style/iconography/app-icon-construction
$sizes = 16, 20, 24, 30, 32, 36, 40, 48, 60, 64, 72, 80, 96, 256

# Create a place to store Png images.
New-Item -ItemType 'directory' -Path $temp -Force

# Create Png images.
Foreach ($size in $sizes)
{
	inkscape $inputSvg --export-filename="$temp\$size.png" -w $size -h $size
}

# Create an Ico file from the images.
$images = @()
Foreach ($size in $sizes)
{
	$images += "$temp\$size.png"
}
convert $images $outputIco

# Remove the temporary directory.
Remove-Item -Path $temp -Recurse
