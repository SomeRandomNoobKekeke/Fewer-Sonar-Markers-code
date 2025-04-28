rmdir /s /q "C:\Users\user\Desktop\c#\mods\FewerSonarMarkers\ClientProject\ClientSource"
xcopy /s /Y /i "CSharp\Client" "C:\Users\user\Desktop\c#\mods\FewerSonarMarkers\ClientProject\ClientSource"

rmdir /s /q "C:\Users\user\Desktop\c#\mods\FewerSonarMarkers\SharedProject\SharedSource"
xcopy /s /Y /i "CSharp\Shared" "C:\Users\user\Desktop\c#\mods\FewerSonarMarkers\SharedProject\SharedSource"

rmdir /s /q "C:\Users\user\Desktop\c#\mods\FewerSonarMarkers\ServerProject\ServerSource"
xcopy /s /Y /i "CSharp\Server" "C:\Users\user\Desktop\c#\mods\FewerSonarMarkers\ServerProject\ServerSource"

cd "C:\Users\user\Desktop\c#\mods\FewerSonarMarkers"
call "Build.cmd"
