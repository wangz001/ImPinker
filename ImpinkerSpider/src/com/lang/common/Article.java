package com.lang.common;

/**
 * article 表和articleSnap两个表的内容
 * 
 * @author wangzheng1
 * 
 */
public class Article {

	/**
	 * 文章id
	 */
	public long Id;
	/**
	 * 名称
	 */
	public String Title;
	/**
	 * 关键词
	 */
	public String KeyWord;
	/**
	 * 文章链接
	 */
	public String Url;
	/**
	 * 简介，关键短语
	 */
	public String Description;
	/**
	 * 来源
	 */
	public String Company;

	/**
	 * //文章发布时间
	 */
	public String PublishTime;

	/**
	 * 第一张图片
	 */
	public String SnapFirstImageUrl;
	/**
	 * keyword标签
	 */
	public String SnapKeyWords;
	/**
	 * description标签
	 */
	public String SnapDescription;
	/**
	 * 正文内容
	 */
	public String SnapContent;

	public long getId() {
		return Id;
	}

	public void setId(long id) {
		Id = id;
	}

	public String getTitle() {
		return Title;
	}

	public void setTitle(String title) {
		Title = title;
	}

	public String getKeyWord() {
		return KeyWord;
	}

	public void setKeyWord(String keyWord) {
		KeyWord = keyWord;
	}

	public String getUrl() {
		return Url;
	}

	public void setUrl(String url) {
		Url = url;
	}

	public String getDescription() {
		return Description;
	}

	public void setDescription(String description) {
		Description = description;
	}

	public String getCompany() {
		return Company;
	}

	public void setCompany(String company) {
		Company = company;
	}

	public String getPublishTime() {
		return PublishTime;
	}

	public void setPublishTime(String publishTime) {
		PublishTime = publishTime;
	}

	public String getSnapFirstImageUrl() {
		return SnapFirstImageUrl;
	}

	public void setSnapFirstImageUrl(String snapFirstImageUrl) {
		SnapFirstImageUrl = snapFirstImageUrl;
	}

	public String getSnapKeyWords() {
		return SnapKeyWords;
	}

	public void setSnapKeyWords(String snapKeyWords) {
		SnapKeyWords = snapKeyWords;
	}

	public String getSnapDescription() {
		return SnapDescription;
	}

	public void setSnapDescription(String snapDescription) {
		SnapDescription = snapDescription;
	}

	public String getSnapContent() {
		return SnapContent;
	}

	public void setSnapContent(String snapContent) {
		SnapContent = snapContent;
	}

}
