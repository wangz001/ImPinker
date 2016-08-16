package com.lang.common;

import org.apache.solr.client.solrj.SolrClient;
import org.apache.solr.client.solrj.impl.HttpSolrClient;

public class SolrJUtil {

	String url = "http://localhost:8983/solr/";
	String coreName="impinker";
	SolrClient client;
	public SolrJUtil(){
		client = new HttpSolrClient(url+coreName);
		
	}
}
