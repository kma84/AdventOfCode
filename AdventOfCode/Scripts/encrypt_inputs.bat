::
: Script to encrypt input files
: P1 - Project directory
::

@echo off

for /f %%f in ('where /r %1 input.txt') do (
  for /F %%A in ("%%f") do (
     if %%~zA gtr 0 (
       for /F %%B in ("%%f.gpg") do (
         if %%~zB equ 0 (
           echo Generating the encrypted file for %%f
           gpg --verbose --symmetric --pinentry-mode=loopback --passphrase %AOC_INPUTS% --yes --cipher-algo AES256 %%f
         )
       )
     )
   ) 
)