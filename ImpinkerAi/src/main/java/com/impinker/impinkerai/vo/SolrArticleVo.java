package com.impinker.impinkerai.vo;


import lombok.Data;
import org.apache.solr.client.solrj.beans.Field;

import java.util.Date;

/**
 * solr 查询pojo类
 */
@Data
public class SolrArticleVo {

    @Field("id")
    private String id;
    @Field("UserId")
    private String userid;
    @Field("Company")
    private String company;
    @Field("ArticleName")
    private String articlename;
    @Field("KeyWords")
    private String keywords;
    @Field("Content")
    private String content;
    @Field("CoverImage")
    private String coverimage;
    @Field("Url")
    private String url;
    @Field("Description")
    private String description;
    @Field("CreateTime")
    private Date createtime;
    @Field("UpdateTime")
    private Date updatetime;

}
