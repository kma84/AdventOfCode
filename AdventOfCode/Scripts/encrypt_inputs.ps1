<#
  Script to encrypt input files
#>

$inputs = Get-ChildItem -Recurse -Path Year* -Include input.txt | % { $_.FullName }

foreach ($input in $inputs)
{
    if (![String]::IsNullOrWhiteSpace((Get-content $input)))
    {
        $encryptedfile = $input + ".gpg"

        if (!(Test-Path $encryptedfile -PathType Leaf) -or [String]::IsNullOrWhiteSpace((Get-content $encryptedfile)))
        {
            write-host "Generating the encrypted file for"$input
            gpg --verbose --symmetric --pinentry-mode=loopback --passphrase $Env:AOC_INPUTS --yes --cipher-algo AES256 $input
        }
    }
}