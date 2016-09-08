package com.lang.interfac;

import java.util.List;

import us.codecraft.webmagic.Page;

public interface MotorXPathInterface {

	/**
	 * 获取title
	 * 
	 * @param page
	 * @return
	 */
	public String getTitleString(Page page);

	/**
	 * 获取首张图片
	 * 
	 * @param page
	 * @return
	 */
	public String getFirstImg(Page page);

	/**
	 * 获取关键字
	 * 
	 * @param page
	 * @return
	 */
	public String getKeyWordString(Page page);

	/**
	 * description 简介
	 * 
	 * @param page
	 * @return
	 */
	public String getDescription(Page page);

	/**
	 * 获取正文内容
	 * 
	 * @param page
	 * @return
	 */
	public String getContentString(Page page);

	/**
	 * 获取文章发布时间
	 * 
	 * @param page
	 * @return
	 */
	public String getPublishTime(Page page);

	/**
	 * 获取url。对url作一些处理。去除锚点等
	 * 
	 * @param page
	 * @return
	 */
	public String getUrl(Page page);

	/**
	 * 判断文章是否有分页
	 * 
	 * @param page
	 * @return
	 */
	public boolean isPagination(Page page);

	/**
	 * 获取文章key
	 * 
	 * @param page
	 * @return
	 */
	public String getPageKey(Page page);

	/**
	 * 获取页码
	 * 
	 * @param page
	 * @return
	 */
	public String getPageIndex(Page page);

	/**
	 * 获取总共有多少页
	 * 
	 * @param page
	 * @return
	 */
	public List<String> getAllPageUrls(Page page);
}
