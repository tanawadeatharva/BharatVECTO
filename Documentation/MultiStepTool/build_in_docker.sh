docker build -t ubuntu_pandoc .

SCRIPT=$(readlink -f "$0")
# Absolute path this script is in, thus /home/user/bin
SCRIPTPATH=$(dirname "$SCRIPT")
echo $SCRIPTPATH

docker run --rm --volume $SCRIPTPATH:/documentation --entrypoint /documentation/convert.sh ubuntu_pandoc
