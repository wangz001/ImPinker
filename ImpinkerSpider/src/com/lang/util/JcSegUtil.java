package com.lang.util;

import java.io.IOException;
import java.io.StringReader;
import java.util.List;

import org.apache.log4j.Logger;
import org.lionsoul.jcseg.extractor.SummaryExtractor;
import org.lionsoul.jcseg.extractor.impl.TextRankKeyphraseExtractor;
import org.lionsoul.jcseg.extractor.impl.TextRankKeywordsExtractor;
import org.lionsoul.jcseg.extractor.impl.TextRankSummaryExtractor;
import org.lionsoul.jcseg.tokenizer.SentenceSeg;
import org.lionsoul.jcseg.tokenizer.core.ADictionary;
import org.lionsoul.jcseg.tokenizer.core.DictionaryFactory;
import org.lionsoul.jcseg.tokenizer.core.ISegment;
import org.lionsoul.jcseg.tokenizer.core.JcsegException;
import org.lionsoul.jcseg.tokenizer.core.JcsegTaskConfig;
import org.lionsoul.jcseg.tokenizer.core.SegmentFactory;

import com.lang.main.MyWebMagic;

public class JcSegUtil {

	public static void main(String[] args) {
		System.out.println("pageIndex=");
		testGetKey();
		GetKeyphrase("");
		GetSummary("", 64);
		for (int i = 0; i < 3; i++) {
			JcSegUtil.GetKeyWords("");
		}
	}

	private static Logger logger = Logger.getLogger(MyWebMagic.class);
	private static ISegment seg;

	/**
	 * 静态块，初始化词典等
	 */
	static {
		JcsegTaskConfig config = new JcsegTaskConfig(true);
		config.setClearStopwords(true); // 设置过滤停止词
		config.setAppendCJKSyn(true); // 设置关闭同义词追加
		config.setKeepUnregWords(true); // 设置去除不识别的词条
		ADictionary dic = DictionaryFactory.createSingletonDictionary(config);
		try {
			dic.loadClassPath();// //加载classpath路径下的全部词库文件的全部词条（默认路径/lexicon）
			seg = SegmentFactory.createJcseg(JcsegTaskConfig.COMPLEX_MODE,
					new Object[] { config, dic });
		} catch (JcsegException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			logger.error(e);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			logger.error(e);
		}
	}

	/**
	 * 关键词提取
	 * 
	 * @param text
	 * @return
	 */
	public static List<String> GetKeyWords(String text) {
		try {
			// 2, 构建TextRankKeywordsExtractor关键字提取器
			TextRankKeywordsExtractor extractor = new TextRankKeywordsExtractor(
					(ISegment) seg);
			extractor.setMaxIterateNum(100); // 设置pagerank算法最大迭代次数，非必须，使用默认即可
			extractor.setWindowSize(5); // 设置textRank计算窗口大小，非必须，使用默认即可
			extractor.setKeywordsNum(10); // 设置最大返回的关键词个数，默认为10

			// 3, 从一个输入reader输入流中获取关键字
			List<String> keywords = extractor
					.getKeywords(new StringReader(text));
			// 4, output:
			return keywords;
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			logger.error(e);
		}
		return null;
	}

	/**
	 * 提取摘要
	 * 
	 * @param text
	 * @return
	 */
	public static String GetSummary(String text, int length) {
		try {
			// 2, 构造TextRankSummaryExtractor自动摘要提取对象
			SummaryExtractor extractor = new TextRankSummaryExtractor(seg,
					new SentenceSeg());

			// 3, 从一个Reader输入流中获取length长度的摘要
			String summary = extractor.getSummary(new StringReader(text),
					length);
			// 4, output:
			return summary;
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			logger.error(e);
		}
		return "";
	}

	/**
	 * 提取关键短语
	 * 
	 * @param text
	 * @return
	 */
	public static List<String> GetKeyphrase(String text) {
		List<String> keyphrases = null;
		try {
			// 2, 构建TextRankKeyphraseExtractor关键短语提取器
			TextRankKeyphraseExtractor extractor = new TextRankKeyphraseExtractor(
					seg);
			extractor.setMaxIterateNum(100); // 设置pagerank算法最大迭代词库，非必须，使用默认即可
			extractor.setWindowSize(5); // 设置textRank窗口大小，非必须，使用默认即可
			extractor.setKeywordsNum(15); // 设置最大返回的关键词个数，默认为10
			extractor.setMaxWordsNum(5); // 设置最大短语词长，默认为5
			// 3, 从一个输入reader输入流中获取短语
			keyphrases = extractor.getKeyphrase(new StringReader(text));
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			logger.error(e);
		}
		return keyphrases;
	}

	private static void testGetKey() {
		// 1, 创建Jcseg ISegment分词对象
		// 默认情况下可以在jcseg-core-{version}.jar同目录下来放一份jcseg.properties来自定义配置。
		JcsegTaskConfig config = new JcsegTaskConfig(true);
		config.setClearStopwords(true); // 设置过滤停止词
		config.setAppendCJKSyn(true); // 设置关闭同义词追加
		config.setKeepUnregWords(true); // 设置去除不识别的词条
		ADictionary dic = DictionaryFactory.createSingletonDictionary(config);

		try {
			dic.loadClassPath();// //加载classpath路径下的全部词库文件的全部词条（默认路径/lexicon）
			ISegment seg = SegmentFactory.createJcseg(
					JcsegTaskConfig.COMPLEX_MODE, new Object[] { config, dic });

			// 2, 构建TextRankKeywordsExtractor关键字提取器
			TextRankKeywordsExtractor extractor = new TextRankKeywordsExtractor(
					(ISegment) seg);
			extractor.setMaxIterateNum(100); // 设置pagerank算法最大迭代次数，非必须，使用默认即可
			extractor.setWindowSize(5); // 设置textRank计算窗口大小，非必须，使用默认即可
			extractor.setKeywordsNum(10); // 设置最大返回的关键词个数，默认为10

			// 3, 从一个输入reader输入流中获取关键字
			String str = "全新迈特威吊丝是大众T系列旗下吊丝第六代车型迈特威。迈特威系列首款吊丝车型T1的设计灵感来自一位荷兰车商在大众工厂看到的运输平板车。作为T系列首款车型，T1承担着经济复苏时期爆发式增长的运输任务，凭借运输性能和可靠性获得了良好的口碑，圆润可爱的造型受到了不少改装发烧友和汽车收藏家的偏爱。自诞生至今，T系列车型共售出了1200余万辆。";
			List<String> keywords = extractor
					.getKeywords(new StringReader(str));

			// 4, output:
			// "分词","方法","分为","标注","相结合","字符串","匹配","过程","大类","单纯"
			for (int i = 0; i < keywords.size(); i++) {
				System.out.println(keywords.get(i));
			}

		} catch (JcsegException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}
