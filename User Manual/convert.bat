@echo off

echo Starting Help-file generation...


setlocal enabledelayedexpansion enableextensions
set LIST=
for /f %%f in (files.txt) do set LIST=!LIST! "%%f"


pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --section-divs -c style.css -B header.html -A footer.html -o output\help.html --mathjax=mathjax.js -H jquery-1.11.3.min.js -H jquery-ui-1.11.4.min.js -H include.js

REM pandoc %LIST% -s -S --toc --toc-depth=2 --katex=katex/katex.min.js --katex-stylesheet=katex/katex.min.css -c style.css -o output\html\help.html
REM pandoc %LIST% -s -S --toc --toc-depth=2 --katex -c style.css -o output\html\help.html
REM pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --section-divs -c style.css -o output\help.html --webtex -H jquery-1.11.3.min.js -H jquery-ui-1.11.4.min.js -H include.js
REM pandoc %LIST% -s -S --toc --toc-depth=2 --self-contained --section-divs -c style.css -o output\help.html --webtex="https://latex.codecogs.com/svg.latex?\large " -H jquery-1.11.3.min.js -H jquery-ui-1.11.4.min.js -H include.js


echo Generated outputfile: output\help.html




REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.docx
REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.pdf
REM pandoc -s -S --toc --toc-depth=2 -N %LIST% -o help.latex



pause