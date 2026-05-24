# Removes XML comments that are not directly attached to a declaration (fixes CS1587/CS1570).
param([string]$Root = (Join-Path $PSScriptRoot '..' 'src'))

function Get-NextNonEmpty([string[]]$lines, [int]$from) {
    for ($i = $from; $i -lt $lines.Count; $i++) {
        if (-not [string]::IsNullOrWhiteSpace($lines[$i])) { return $i }
    }
    return -1
}

function Get-PrevNonEmpty([string[]]$lines, [int]$from) {
    for ($i = $from; $i -ge 0; $i--) {
        if (-not [string]::IsNullOrWhiteSpace($lines[$i])) { return $i }
    }
    return -1
}

Get-ChildItem -Path $Root -Filter '*.cs' -Recurse | ForEach-Object {
    $path = $_.FullName
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.AddRange([IO.File]::ReadAllLines($path))
    $changed = $false

    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i].TrimStart() -notmatch '^///') { continue }

        $nextIdx = Get-NextNonEmpty $lines ($i + 1)
        if ($nextIdx -lt 0) { continue }
        $next = $lines[$nextIdx].Trim()

        if ($next -eq '{' -or $next -eq '}' -or $next.StartsWith('///')) {
            $lines.RemoveAt($i)
            $changed = $true
            $i--
            continue
        }

        $prevIdx = Get-PrevNonEmpty $lines ($i - 1)
        if ($prevIdx -ge 0) {
            $prev = $lines[$prevIdx].TrimEnd()
            if ($prev.EndsWith('{') -or $prev.EndsWith('(') -or $prev.EndsWith(',')) {
                $lines.RemoveAt($i)
                $changed = $true
                $i--
            }
        }
    }

    if ($changed) {
        [IO.File]::WriteAllLines($path, $lines)
        Write-Host "Fixed $path"
    }
}
