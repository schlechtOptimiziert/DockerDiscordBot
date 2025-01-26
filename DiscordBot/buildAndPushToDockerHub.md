#build for arm64 for raspi
docker buildx create --use
docker buildx build --platform linux/arm64 -t <username>/<appname> . --load

docker push <username>/<appname>