package com.lang.common;

import java.util.List;

import us.codecraft.webmagic.Page;

public class MutilePageModel {

	/**
	 * 文章主键。每个文章的各个分页，主键一致
	 */
	private String pageKey;

	/**
	 * 页码
	 */
	private String pageindex;

	/**
	 * 全部页
	 */
	private List<String> allPages;

	/**
	 * 当前页
	 */
	private Page page;

	/**
	 * 已经下载的页码url
	 */
	private List<String> donePages;

	public List<String> getAllPages() {
		return allPages;
	}

	public void setAllPages(List<String> allPages) {
		this.allPages = allPages;
	}

	public List<String> getDonePages() {
		return donePages;
	}

	public void setDonePages(List<String> donePages) {
		this.donePages = donePages;
	}

	public String getPageKey() {
		return pageKey;
	}

	public void setPageKey(String pageKey) {
		this.pageKey = pageKey;
	}

	public String getPageindex() {
		return pageindex;
	}

	public void setPageindex(String pageindex) {
		this.pageindex = pageindex;
	}

	public List<String> getOtherPages() {
		return allPages;
	}

	public void setOtherPages(List<String> otherPages) {
		this.allPages = otherPages;
	}

	public Page getPage() {
		return page;
	}

	public void setPage(Page page) {
		this.page = page;
	}

}
