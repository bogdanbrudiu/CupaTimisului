FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y cron unzip wget

WORKDIR /tmp
RUN wget -q --no-check-certificate https://downloads.rclone.org/rclone-current-linux-amd64.zip 
RUN unzip rclone-current-linux-amd64.zip \
	&& mv /tmp/rclone-*-linux-amd64/rclone /usr/bin \
    && rm -r /tmp/rclone*

WORKDIR .
RUN chown root:root /usr/bin/rclone
RUN chmod -v +x /usr/bin/rclone

COPY Submitter/rclone-cron.sh /rclone-cron.sh

RUN chown root:root /rclone-cron.sh
RUN chmod -v +x /rclone-cron.sh

RUN crontab -l | { cat; echo "* * * * * rclone-cron.sh && curl -fsS --retry 3 https://hc-ping.com/651d1006-c30f-48be-b7cd-ee85499d1ac6 > /dev/null"; } | crontab -




FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Submitter/Submitter.csproj", "Submitter/"]
RUN dotnet restore "Submitter/Submitter.csproj"
COPY . .
WORKDIR "/src/Submitter"
RUN dotnet build "Submitter.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Submitter.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Submitter.dll"]