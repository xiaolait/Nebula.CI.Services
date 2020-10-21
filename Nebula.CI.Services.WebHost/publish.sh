#! /bin/bash

rm -r ./wwwroot/*

cd ./ClientApp && \
  cp ./static/antd.css ./node_modules/ant-design-vue/dist && \
  cp ./static/codemirror.css ./node_modules/codemirror/lib && \
  npm run build && cp -r ./dist/* ../wwwroot/ && cd ..

dotnet publish -o ./bin/Release
docker build -t nebula/ci/services .