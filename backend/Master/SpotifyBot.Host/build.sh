set -e

rm -rf build
dotnet publish --configuration Release --runtime win-x64 --output build
dotnet restore # dotnet publish breaks dependencies

cp config.json build
cp data.sqlite3 build

cd ../frontend
yarn nps webpack.build.development && mv dist ../backend/build/wwwroot/
cd -

rm -rf SpotifyBot.zip
zip -r SpotifyBot.zip build
