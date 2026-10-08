param(
    [string]$Destination,
    [ValidatePattern('^/(?:[A-Za-z0-9._~-]+/)*$')]
    [string]$BasePath = '/lab/'
)
# Собираю статическую лабораторию с явным путём размещения.
$ErrorActionPreference = 'Stop'
if (($BasePath -split '/') -contains '..' -or ($BasePath -split '/') -contains '.') {
    throw 'BasePath must not contain relative path segments.'
}
$repoDirectory = $PSScriptRoot
if (-not $Destination) { $Destination = Join-Path $repoDirectory 'artifacts/browser-site' }
$Destination = [System.IO.Path]::GetFullPath($Destination)
$publishDirectory = Join-Path $repoDirectory 'artifacts/browser-publish'
dotnet publish (Join-Path $repoDirectory 'ITMO.SymbolicComputations.Browser/ITMO.SymbolicComputations.Browser.csproj') -c Release -o $publishDirectory --disable-build-servers -m:1
if ($LASTEXITCODE -ne 0) { throw 'Browser publish failed.' }
$labDirectory = Join-Path $Destination 'lab'
New-Item -ItemType Directory -Force -Path $labDirectory | Out-Null
Copy-Item -Path (Join-Path $publishDirectory 'wwwroot/*') -Destination $labDirectory -Recurse -Force
$interfaceDirectory = Join-Path $repoDirectory 'ITMO.SymbolicComputations.Web/wwwroot'
Copy-Item -Path (Join-Path $interfaceDirectory '*') -Destination $labDirectory -Recurse -Force
$page = [System.IO.File]::ReadAllText((Join-Path $interfaceDirectory 'index.html'))
$page = $page.Replace('<script src="./app.js" defer></script>', '<script src="symbolic-transport.js"></script>' + "`n" + '  <script src="./app.js" defer></script>')
[System.IO.File]::WriteAllText((Join-Path $labDirectory 'index.html'), $page, [System.Text.UTF8Encoding]::new($false))
foreach ($htmlFile in Get-ChildItem -LiteralPath $labDirectory -Filter '*.html' -File) {
    $html = [System.IO.File]::ReadAllText($htmlFile.FullName)
    $baseElement = '<base href="' + $BasePath + '">'
    if ($html -match '<base\s+href="[^"]*"\s*/?>') {
        $html = [regex]::Replace($html, '<base\s+href="[^"]*"\s*/?>', $baseElement)
    } else {
        $html = $html.Replace('<head>', '<head>' + "`n  " + $baseElement)
    }

    # Fingerprinted URLs prevent an older UI script/style being reused with a newer engine.
    $html = [regex]::Replace($html, '(src|href)="((?:\./)?(?:app|fractal|styles|symbolic-transport)\.(?:js|css))"', {
        param($match)
        $assetPath = Join-Path $labDirectory $match.Groups[2].Value
        $hash = (Get-FileHash -LiteralPath $assetPath -Algorithm SHA256).Hash.Substring(0, 12).ToLowerInvariant()
        return $match.Groups[1].Value + '="' + $match.Groups[2].Value + '?v=' + $hash + '"'
    })
    [System.IO.File]::WriteAllText($htmlFile.FullName, $html, [System.Text.UTF8Encoding]::new($false))
}
$rootIndex = Join-Path $Destination 'index.html'
if (-not (Test-Path -LiteralPath $rootIndex)) {
    $landing = '<!doctype html><html lang="ru"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><meta http-equiv="refresh" content="0;url=./lab/"><title>Symbolic — лаборатория выражений</title></head><body><a href="./lab/">Открыть лабораторию выражений</a></body></html>'
    [System.IO.File]::WriteAllText($rootIndex, $landing, [System.Text.UTF8Encoding]::new($false))
}
[System.IO.File]::WriteAllText((Join-Path $Destination '.nojekyll'), '')
# The host's compressed HTML belongs to its diagnostic page, not the final interface.
foreach ($suffix in '.br', '.gz') {
    $compressedIndex = Join-Path $labDirectory ('index.html' + $suffix)
    if (Test-Path -LiteralPath $compressedIndex) { Remove-Item -LiteralPath $compressedIndex }
}
Write-Output "Browser application built: $labDirectory"
Write-Output "Configured public base path: $BasePath"
