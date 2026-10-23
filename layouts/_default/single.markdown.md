# {{ .Title }}

{{ with .Description }}> {{ . }}

{{ end }}
{{- $content := .RenderShortcodes -}}
{{- if eq .Section "cmdlets" -}}
  {{- $content = replaceRE `\]\(\.\./([^/]+)/(?:(#[^)]+))?\)` `](${1}.md${2})` $content -}}
{{- end -}}
{{ $content }}
