package com.lang.common;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.List;

import org.apache.solr.client.solrj.impl.HttpSolrServer;
import org.apache.solr.common.SolrInputDocument;

import com.lang.util.TUtil;

public class SolrJUtil {

	static String SOLR_URL = "http://localhost:8080/solr/";
	// static String SOLR_URL = "http://101.200.175.157:8080/solr/";
	static String coreName = "impinker";
	static List<Article> articleLists = new ArrayList<Article>();

	private volatile static SolrJUtil solrJUtil = null;

	/**
	 * 单利模式，获取实例
	 * 
	 * @return
	 */
	public static SolrJUtil getInstance() {
		if (solrJUtil == null) {
			synchronized (SolrJUtil.class) { // 加锁，解决多线程使用单例的问题
				solrJUtil = new SolrJUtil();
			}
		}
		return solrJUtil;
	}

	private SolrJUtil() {

	}

	public void AddDocs(Article article) {
		synchronized (ArticleUrlCache.class) {
			if (article != null) {
				articleLists.add(article);
			}
			// 每次添加100条索引
			if (articleLists.size() >= 100) {
				AddDocs(articleLists);
				articleLists.clear();
			}
		}
	}

	/**
	 * 所有线程执行完后，把list里剩余的数据提交
	 */
	public void LastCommit() {
		if (articleLists.size() > 0) {
			AddDocs(articleLists);
			articleLists.clear();
		}
	}

	/**
	 * 批量添加索引
	 * 
	 * @param articleLists
	 */
	private static void AddDocs(List<Article> articleLists) {
		HttpSolrServer server = new HttpSolrServer(SOLR_URL + coreName);
		Collection<SolrInputDocument> docs = new ArrayList<SolrInputDocument>();

		String time = TUtil.getUTCTime(new Date().getTime());
		for (Article article : articleLists) {
			SolrInputDocument doc1 = new SolrInputDocument();
			doc1.addField("id", "travels_" + article.getId(), 1.0f);
			doc1.addField("UserId", AppStart.AdminUserId, 1.0f);
			doc1.addField("ArticleName", article.getTitle());
			doc1.addField("KeyWords", article.getKeyWord());
			doc1.addField("Description", article.getDescription());
			doc1.addField("Url", article.getUrlString());
			doc1.addField("CreateTime", article.getCreateTime());
			doc1.addField("UpdateTime", time);
			doc1.addField("CoverImage", article.getCoverImage());
			doc1.addField("content", article.getContent());
			docs.add(doc1);
		}
		try {

			// 可以通过三种方式增加docs,其中server.add(docs.iterator())效率最高
			// 增加后通过执行commit函数commit (936ms)
			// server.add(docs);
			// server.commit();

			// 增加doc后立即commit (946ms)
			// UpdateRequest req = new UpdateRequest();
			// req.setAction(ACTION.COMMIT, false, false);
			// req.add(docs);
			// UpdateResponse rsp = req.process(server);

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
