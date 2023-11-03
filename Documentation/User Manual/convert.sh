#!/bin/bash


#readarray [-d delim] [-n count] [-O origin] [-s count]
 #   [-t] [-u fd] [-C callback] [-c quantum] [array]
#ls

cd documentation

#ls

declare -a FILES
mapfile -t FILES < files.txt
cat files.txt

echo "\n"
echo "Running pandoc with these input files"
echo "${FILES[@]}"


OUTPUTFILE="help.html"
echo "Start conversion"
echo "In case of an error check if the line endings are set to LF"

echo "Prerequesites  mermaid-pandoc filter, requires the chromium snap package and libgbm1 to work " 
#pandoc --verbose  -f markdown+raw_html %LIST% -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -o HashingToolHelp.html  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js
#pandoc --verbose -f markdown+raw_html+smart ${FILES[@]} -s --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --webtex -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -o $OUTPUTFILE  -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js

pandoc --verbose -f markdown+raw_html -F mermaid-filter -s --toc --toc-depth=2 --self-contained --section-divs --mathjax -c includes/style.css -c includes/print.css -B includes/header.html -A includes/footer.html -H includes/jquery.js -H includes/jquery-ui.js -H includes/include.js -o help.html  -V "pagetitle:VECTO xEV User Manual" ${FILES[@]} 


