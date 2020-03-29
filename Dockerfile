FROM golang:1.14 AS builder

WORKDIR /build
COPY . .
RUN go build -ldflags="-s -w"
RUN ls -lh

FROM mcr.microsoft.com/dotnet/core/sdk:3.1
COPY --from=builder /build/resharper-action /usr/bin
