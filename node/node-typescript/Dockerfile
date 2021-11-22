# Build and run sample as a docker multistage build
# sudo docker build -t canary-typescript-node:multi -f src/node-typescript/Dockerfile.multistage src/node-typescript
# docker run -it canary-typescript-node:multi

FROM node:carbon-stretch AS builder
WORKDIR /opt/work
COPY . .
RUN npm install
RUN npm test

FROM node:carbon-alpine
WORKDIR /opt/app
COPY --from=builder /opt/work .
CMD node sample.js