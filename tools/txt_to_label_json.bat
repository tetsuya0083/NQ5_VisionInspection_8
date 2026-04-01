@echo off
chcp 65001 > nul
cd /d "%~dp0"
py txt_to_label_json.py
pause
