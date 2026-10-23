{{- $image := partial "theme-image-params.html" . -}}
{{- $src := or $image.src $image.light -}}
![{{ $image.alt }}]({{ $src | absURL }})
