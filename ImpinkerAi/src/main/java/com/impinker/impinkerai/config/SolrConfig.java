package com.impinker.impinkerai.config;

import lombok.Data;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

@Data
@ConfigurationProperties(prefix = "solrserver")
@Component
public class SolrConfig {

    private String solrpath;
}
