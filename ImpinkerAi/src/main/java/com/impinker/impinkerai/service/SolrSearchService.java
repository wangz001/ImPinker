package com.impinker.impinkerai.service;

import com.impinker.impinkerai.config.SolrConfig;
import com.impinker.impinkerai.vo.SolrArticleVo;
import org.apache.solr.client.solrj.SolrClient;
import org.apache.solr.client.solrj.SolrQuery;
import org.apache.solr.client.solrj.impl.HttpSolrClient;
import org.apache.solr.client.solrj.response.QueryResponse;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class SolrSearchService {

    @Autowired
    private SolrConfig solrConfig;

    //http://localhost:8080/solr/impinker";

    public String search(String keyword) {
        return "aaa";
    }

    public List<SolrArticleVo> search(String keywords, Integer page, Integer rows) throws Exception {
        String solrServer = solrConfig.getSolrpath();
        SolrClient solr = new HttpSolrClient(solrServer);
        SolrQuery solrQuery = new SolrQuery(); //构造搜索条件
        solrQuery.setQuery("ArticleName:" + keywords); //搜索关键词
        // 设置分页 start=0就是从0开始，，rows=5当前返回5条记录，第二页就是变化start这个值为5就可以了。
        solrQuery.setStart((Math.max(page, 1) - 1) * rows);
        solrQuery.setRows(rows);

        // 执行查询
        QueryResponse queryResponse = solr.query(solrQuery);
        List<SolrArticleVo> foos = queryResponse.getBeans(SolrArticleVo.class);


        return foos;
    }

}
