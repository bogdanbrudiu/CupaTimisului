#!/bin/bash
[ "${FLOCKER}" != "$0" ] && exec env FLOCKER="$0" flock -en "$0" "$0" "$@" || echo $(date -u)' | rclone-cron.sh already running.' >> $CRONLOG 

#### rclone sync options

SRC=/app/wwwroot/Uploads

#---- Edit this to the desired destination
DEST=GoogleDrive:

#---- Location of sync log [will be rotated with savelog]
LOGFILE=$SRC/.rclone/rclone-sync.log
LOGS='-vv --log-file='$LOGFILE

#---- Location of cron log
CRONLOG=$SRC/.rclone/rclone-cron.log

#Rotate logs.
savelog -n -c 7 $LOGFILE >> $CRONLOG

#Log startup
echo $(date -u)' | starting rclone-cron.sh . . .' >> $CRONLOG

#Now do the sync!
rclone sync $SRC $DEST --transfers=2 --checkers=4 --min-age=1m --exclude-from $SRC/.rclone/.rclone-ignore  --exclude-if-present .rclone-ignore --delete-excluded $LOGS

#log success
echo $(date -u)' | completed rclone-cron.sh.' >> $CRONLOG

exit

