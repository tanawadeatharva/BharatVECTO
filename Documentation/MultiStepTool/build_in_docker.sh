docker build -t ubuntupandoc .
docker run --rm --volume /mnt/c/Users/Harry/source/repos/hm_vecto-dev/Documentation/MultiStepTool:/documentation --entrypoint /documentation/convert.sh ubuntu_pandoc
