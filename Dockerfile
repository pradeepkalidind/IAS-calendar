ARG CONTAINER_REGISTRY=mcr.microsoft.com
FROM ${CONTAINER_REGISTRY}/dotnet/sdk:8.0-alpine AS publish
WORKDIR /src

RUN mkdir -p $HOME/.nuget/

ARG CREDENTIALPROVIDER_URL="https://github.com/Microsoft/artifacts-credprovider/releases/latest/download/Microsoft.NuGet.CredentialProvider.tar.gz"
RUN curl -H "Accept: application/octet-stream" \
     -s \
     -S \
     -L \
     "${CREDENTIALPROVIDER_URL}" | tar xz -C "$HOME/.nuget/" "plugins/netcore"

COPY ./src/calendar-service ./calendar-service
COPY ./src/calendar-authentication ./calendar-authentication
COPY ./src/calendar-client ./calendar-client
COPY ./src/calendar-client-schema ./calendar-client-schema
COPY ./src/calendar-feed ./calendar-feed
COPY ./src/calendar-general ./calendar-general
COPY ./src/calendar-general-dto ./calendar-general-dto
COPY ./src/calendar-model ./calendar-model
COPY ./src/calendar-model-compact ./calendar-model-compact
COPY ./src/calendar-model-convertor ./calendar-model-convertor
COPY ./src/calendar-persistence ./calendar-persistence
COPY ./src/DBMigration ./DBMigration
COPY ./NuGet.Config .

ARG FEED_ACCESSTOKEN
RUN export VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/vialto/myMobility-CoreApp/_packaging/ci-1/nuget/v3/index.json\", \"username\":\"docker\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"  \
    && dotnet publish "/src/calendar-service/calendar-service.csproj" -c Release -o /app/publish \
         --runtime linux-musl-x64 \
         --self-contained true \
         /p:PublishReadyToRun=true

FROM ${CONTAINER_REGISTRY}/dotnet/runtime-deps:8.0-alpine AS final

RUN apk add --no-cache icu-libs ca-certificates

COPY --from=publish ./src/calendar-service/Certificates/server.crt /usr/local/share/ca-certificates/server.crt
RUN /usr/sbin/update-ca-certificates

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

WORKDIR /app
EXPOSE 8080
COPY --chown=app:app --from=publish /app/publish .

ENTRYPOINT ["./Calendar.Service"]
