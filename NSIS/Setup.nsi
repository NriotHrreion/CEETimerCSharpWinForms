; �ýű�ʹ�� HM VNISEdit �ű��༭���򵼲���

; ��װ�����ʼ���峣��
!define PRODUCT_NAME "�߿�����ʱ"
!define PRODUCT_VERSION "v1.9.1"
!define PRODUCT_PUBLISHER "WangHaonie"
!define PRODUCT_WEB_SITE "https://github.com/WangHaonie/CEETimerCSharpWinForms"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKCU"

SetCompressor lzma

; ------ MUI �ִ����涨�� (1.67 �汾���ϼ���) ------
!include "MUI.nsh"

; MUI Ԥ���峣��
!define MUI_ABORTWARNING
!define MUI_ICON "..\CEETimerCSharpWinForms\AppIcon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; ��ӭҳ��
!insertmacro MUI_PAGE_WELCOME
; ���Э��ҳ��
!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
; ��װĿ¼ѡ��ҳ��
!insertmacro MUI_PAGE_DIRECTORY
; ��װ����ҳ��
!insertmacro MUI_PAGE_INSTFILES
; ��װ���ҳ��
!define MUI_FINISHPAGE_RUN "$INSTDIR\CEETimerCSharpWinForms.exe"
!insertmacro MUI_PAGE_FINISH

; ��װж�ع���ҳ��
!insertmacro MUI_UNPAGE_INSTFILES

; ��װ�����������������
!insertmacro MUI_LANGUAGE "SimpChinese"

; ��װԤ�ͷ��ļ�
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI �ִ����涨����� ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Setup.exe"
InstallDir "$PROFILE\AppData\Local\CEETimerCSharpWinForms"
InstallDirRegKey HKCU "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show
BrandingText "Copyright (C) 2023 WangHaonie"

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite on
  File "..\CEETimerCSharpWinForms\bin\x64\Debug\CEETimerCSharpWinForms.exe"
  CreateDirectory "$SMPROGRAMS\�߿�����ʱ"
  CreateShortCut "$SMPROGRAMS\�߿�����ʱ\�߿�����ʱ.lnk" "$INSTDIR\CEETimerCSharpWinForms.exe"
  CreateShortCut "$DESKTOP\�߿�����ʱ.lnk" "$INSTDIR\CEETimerCSharpWinForms.exe"
  File "..\CEETimerCSharpWinForms\bin\x64\Debug\CEETimerCSharpWinForms.exe.config"
  File "..\CEETimerCSharpWinForms\bin\x64\Debug\Newtonsoft.Json.dll"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\�߿�����ʱ\GitHub.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\�߿�����ʱ\ж�� �߿�����ʱ.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\CEETimerCSharpWinForms.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

/******************************
 *  �����ǰ�װ�����ж�ز���  *
 ******************************/

Section Uninstall
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\CEETimerCSharpWinForms.exe.config"
  Delete "$INSTDIR\CEETimerCSharpWinForms.exe"

  Delete "$SMPROGRAMS\�߿�����ʱ\ж�� �߿�����ʱ.lnk"
  Delete "$SMPROGRAMS\�߿�����ʱ\GitHub.lnk"
  Delete "$DESKTOP\�߿�����ʱ.lnk"
  Delete "$SMPROGRAMS\�߿�����ʱ\�߿�����ʱ.lnk"

  RMDir "$SMPROGRAMS\�߿�����ʱ"

  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  SetAutoClose true
SectionEnd

#-- ���� NSIS �ű��༭�������� Function ���α�������� Section ����֮���д���Ա��ⰲװ�������δ��Ԥ֪�����⡣--#

Function un.onInit
  ExecWait '"taskkill" /F /IM "CEETimerCSharpWinForms.exe"'
FunctionEnd

Function un.onUninstSuccess
  HideWindow
FunctionEnd

Function .onInit
  ExecWait '"taskkill" /F /IM "CEETimerCSharpWinForms.exe"'
FunctionEnd

Function .onInstSuccess
  Exec "$INSTDIR\CEETimerCSharpWinForms.exe"
FunctionEnd
