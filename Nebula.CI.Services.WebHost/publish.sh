#! /bin/bash

rm -r ./wwwroot/*
rm -r ./bin

cd ./ClientApp && npm install && \
  cp ./static/antd.css ./node_modules/ant-design-vue/dist && \
  cp ./static/codemirror.css ./node_modules/codemirror/lib && \
  cp ./static/Pipeline.vue ./node_modules/vue-pipeline/src/components && \
  cp ./static/PipelineNodeEnd.vue ./node_modules/vue-pipeline/src/components && \
  cp ./static/PipelineNodeStart.vue ./node_modules/vue-pipeline/src/components && \
  npm run build && cp -r ./dist/* ../wwwroot/ && cd ..

dotnet publish -o ./bin/Release
docker build -t nebula/ci/services .
rm /Users/xiaolait/Projects/docker/tmp/nebula.ci.services.tar
docker save nebula/ci/services > /Users/xiaolait/Projects/docker/tmp/nebula.ci.services.tar