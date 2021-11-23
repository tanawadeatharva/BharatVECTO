docker build -t pandoc/latex/bash .
docker run --rm --volume /mnt/c/Users/Harry/source/repos/hm_vecto-dev/Documentation/MultiStepTool:/data --entrypoint /data/convert.sh pandoc/latex/bash
