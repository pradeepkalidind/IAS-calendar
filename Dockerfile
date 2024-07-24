FROM node:slim

COPY package*.json ./

RUN npm install

COPY index.js .

EXPOSE 8080
CMD [ "node", "index.js" ]