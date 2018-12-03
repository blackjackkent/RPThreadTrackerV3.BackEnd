# Usage: ./InitDatabase.sh {server}
# Example: ./InitDatabase.sh "(localdb)\mssqllocaldb"

SERVER=$1

echo "Initializing database"
sqlcmd -S $SERVER -E -i "000 - Initialize Database.sql"
echo "Adding AspNetUsers"
sqlcmd -S $SERVER -E -i "010 - Add AspNetUsers.sql"
echo "Adding UserSetting"
sqlcmd -S $SERVER -E -i "030 - Add UserSetting.sql"
echo "Adding Platforms"
sqlcmd -S $SERVER -E -i "045 - Add Platforms.sql"
echo "Adding Characters"
sqlcmd -S $SERVER -E -i "050 - Add Characters.sql"
echo "Adding Threads"
sqlcmd -S $SERVER -E -i "070 - Add Threads.sql"
echo "Adding thread tags"
sqlcmd -S $SERVER -E -i "090 - Add Thread Tags.sql"
echo "Adding logging table"
sqlcmd -S $SERVER -E -i "110 - Add Logging Table.sql"
echo "Adding refresh token table"
sqlcmd -S $SERVER -E -i "120 - Add Refresh Token Table.sql"
