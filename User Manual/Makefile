phony:
	@echo "Starting Help-file generation..."
	@pandoc  -f markdown+raw_html $(shell cat files.txt) -s -S --toc --toc-depth=2 --self-contained --email-obfuscation=none --section-divs --mathjax=includes/mathjax.js -c includes/style.css -B includes/header.html -A includes/footer.html -o help.html  -H includes/jquery-1.11.3.min.js -H includes/jquery-ui-1.11.4.min.js -H includes/include.js
	@echo "Generated outputfile: help.html"
