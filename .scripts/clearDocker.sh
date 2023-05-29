# stop all running containers
echo '####################################################'
echo 'Stopping running containers (if available)...'
echo '####################################################'
docker stop $(docker ps -aq --filter "name=functions") || true

# remove all stopped containers
echo '####################################################'
echo 'Removing containers ..'
echo '####################################################'
docker rm $(docker ps -aq --filter "name=functions") || true


# remove all images
echo '####################################################'
echo 'Removing images ...'
echo '####################################################'
docker rmi $(docker images -q --filter "reference=*functions*") || true

# remove all stray volumes if any
#echo '####################################################'
#echo 'Revoming docker container volumes (if any)'
#echo '####################################################'
#docker volume rm $(docker volume ls -q --filter "name=*kassensystembbs2*")
