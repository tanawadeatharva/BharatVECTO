@echo off

echo Starting Help-file generation...


setlocal enabledelayedexpansion enableextensions
set LIST=
for /f %%f in (files.txt) do set LIST=!LIST! "%%f"


pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --mathjax=includes/mathjax.js -c includes/style.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery-1.11.3.min.js -H includes/jquery-ui-1.11.4.min.js -H includes/include.js

REM pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex  -c includes/style.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery-1.11.3.min.js -H includes/jquery-ui-1.11.4.min.js -H includes/include.js
REM pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex="https://latex.codecogs.com/svg.latex?\large "  -c includes/style.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery-1.11.3.min.js -H includes/jquery-ui-1.11.4.min.js -H includes/include.js

echo Generated outputfile: help.html

REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.docx
REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.pdf
REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.latex



pause