set WORKSPACE=.

set GEN_CLIENT="%WORKSPACE%/Luban.ClientServer/Luban.ClientServer"
set CONF_ROOT="%WORKSPACE%/DesignerConfigs"

%GEN_CLIENT% -j cfg --^
 -d %CONF_ROOT%/Defines/__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir ../UnityProject/Assets/Script/Data/Excel/Gen ^
 --output_data_dir ../UnityProject/Assets/StreamingAssets/ExcelData/Json ^
 --gen_types code_cs_unity_json,data_json ^
 -s all 

pause