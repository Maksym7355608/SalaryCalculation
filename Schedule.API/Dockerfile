FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
EXPOSE 8080

ENV TZ 'Europe/Kiev'
    RUN echo $TZ > /etc/timezone && \
    apt-get update && apt-get install -y tzdata && \
    rm /etc/localtime && \
    ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    apt-get clean

COPY . .
ENTRYPOINT ["dotnet", "Schedule.API.dll"]
