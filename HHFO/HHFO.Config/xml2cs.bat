@echo off

rem �f�B���N�g������xml�X�L�[�}����class�t�@�C���̎����������s��
CALL "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat"

rem xml�X�L�[�}��class�t�@�C��
CALL xsd /classes /namespace:HHFO.Config ErrorMessage.xsd /element:ErrorMessage
CALL xsd /classes /namespace:HHFO.Config CommonSetting.xsd /element:CommonSetting
CALL xsd /classes /namespace:HHFO.Config SystemMessage.xsd /element:SystemMessage
CALL xsd /classes /namespace:HHFO.Config UserSetting.xsd /element:UserSetting

PAUSE