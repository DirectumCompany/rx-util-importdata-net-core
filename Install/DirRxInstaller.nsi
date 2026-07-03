;!include MUI2.nsh
LoadLanguageFile "${NSISDIR}\Contrib\Language files\Russian.nlf"
; ����������.
!define TEMP1 $R0 ;Temp variable
; ������������ �����������.
Name "������� ������� ������"
; ���������� ����������.
InstallDir "$PROGRAMFILES64\Directum Company\DirectumRX\ImportData"
; ������������ �����������.
OutFile "Setup.exe"

VIProductVersion 4.1.261086.0
VIAddVersionKey FileVersion 4.1.261086.0
VIAddVersionKey ProductVersion 4.1.261086.0
XPStyle on
; ���������� ����������.
Page directory 
Page instfiles

Section "Components"
 SetOutPath $INSTDIR
 File /r "..\src\ImportData\bin\Debug\net9.0\*.*" 
SectionEnd

