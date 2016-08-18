package com.lang.common;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import org.apache.solr.client.solrj.impl.HttpSolrServer;
import org.apache.solr.client.solrj.response.UpdateResponse;
import org.apache.solr.common.SolrInputDocument;

import com.lang.util.TUtil;

public class SolrJUtil {

	static String SOLR_URL = "http://localhost:8080/solr/";
	static String coreName="impinker";
	static HttpSolrServer server;
	public SolrJUtil(){
		//client = new HttpSolrClient(SOLR_URL+coreName);
		 server = new HttpSolrServer(SOLR_URL+coreName);
	}
	
	public void AddDocs(Article article) {
	    Collection<SolrInputDocument> docs = new ArrayList<SolrInputDocument>();
	      SolrInputDocument doc1 = new SolrInputDocument();
	      doc1.addField("id", "travels_"+article.getId(), 1.0f);
	      doc1.addField("UserId", AppStart.AdminUserId, 1.0f);
	      doc1.addField("ArticleName", article.getTitle());
	      doc1.addField("KeyWords", article.getKeyWord());
	      doc1.addField("Description", article.getDescription());
	      doc1.addField("Url", article.getUrlString());
	      doc1.addField("CreateTime","2016-08-11T08:37:54.87Z");
	      doc1.addField("UpdateTime", "2016-08-11T08:37:54.87Z");
	      doc1.addField("CoverImage", article.getCoverImage());
	      //doc1.addField("Content", article.getContent());
	      docs.add(doc1);
	    try {
	     
	      // 可以通过三种方式增加docs,其中server.add(docs.iterator())效率最高
	      // 增加后通过执行commit函数commit (936ms)
//				 server.add(docs);
//				 server.commit();

	      // 增加doc后立即commit (946ms)
//				 UpdateRequest req = new UpdateRequest();
//				 req.setAction(ACTION.COMMIT, false, false);
//				 req.add(docs);
//				 UpdateResponse rsp = req.process(server);

	      // the most optimal way of updating all your docs 
	      // in one http request(432ms)
	    	server.add(docs.iterator());
	    	 server.commit();
	    } catch (Exception e) {
	      System.out.println(e);
	    }
	  }

	  public static void delDocs() {
	    long start = System.currentTimeMillis();
	    try {
	      HttpSolrServer server = new HttpSolrServer(SOLR_URL);
	      List<String> ids = new ArrayList<String>();
	      for (int i = 1; i < 300; i++) {
	        ids.add("id" + i);
	      }
	      server.deleteById(ids);
	      server.commit();
	    } catch (Exception e) {
	      System.out.println(e);
	    }
	    System.out.println("time elapsed(ms):"
	        + (System.currentTimeMillis() - start));
	  }
}
