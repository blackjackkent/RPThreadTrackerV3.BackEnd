/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/000 - Initialize Database.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/010 - Add AspNetUsers.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/030 - Add UserSetting.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/045 - Add Platforms.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/050 - Add Characters.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/070 - Add Threads.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/090 - Add Thread Tags.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/110 - Add Logging Table.sql"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Passw@rd' -i "/opt/scripts/120 - Add Refresh Token Table.sql"
