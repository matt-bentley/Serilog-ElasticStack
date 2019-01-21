# https://github.com/elastic/logstash-docker
FROM docker.elastic.co/logstash/logstash-oss:6.5.4
RUN logstash-plugin install logstash-input-http
RUN logstash-plugin install logstash-input-beats

COPY ./pipeline /usr/share/logstash/pipeline

# Add your logstash plugins setup here
# Example: RUN logstash-plugin install logstash-filter-json
