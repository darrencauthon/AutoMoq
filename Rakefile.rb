# Rakefile.rb
require 'rubygems'      
require 'ftools'

ILMERGE = "ilmerge"
DEVENV = "\"C:\\Program Files\\Microsoft Visual Studio 9.0\\Common7\\IDE\\devenv.exe\""

SOLUTION_ROOT = Dir.pwd
SOLUTION_FILE = "\"#{SOLUTION_ROOT}\\automoq\\automoq.sln\""

BIN_DIRECTORY = "#{SOLUTION_ROOT}\\AutoMoq\\AutoMoq\\bin\\Release"

DEPLOY_DIRECTORY = "#{SOLUTION_ROOT}\\Deploy"
DEPLOY_FILE =  "#{DEPLOY_DIRECTORY}\\AutoMoq.dll"

task :default=>[:say_hi]

desc "Says Hi"
task :say_hi do 
	puts "Hi there. Rake is working."
	puts "Type rake --tasks."
end

desc "Count files of each type in this solution"
task :file_count do
	report = {}
	Dir['./**/**'].each do |f| 
		report[File.extname(f)].nil? ? report[File.extname(f)] = 1 : report[File.extname(f)] += 1 
	end
	report.each do |type, count|
		puts "#{type}: #{count}"
	end   
end

desc "Clean the solution"
task :clean do
	sh "#{DEVENV} #{SOLUTION_FILE} /clean"
end

desc "Execute release build"
task :release_build do
	sh "#{DEVENV} #{SOLUTION_FILE} /build Release"
end

desc "Create the AutoMoq.dll in /Deploy"
task :create_deployment do
    Rake::Task['release_build'].execute
	mkdir "#{DEPLOY_DIRECTORY}" unless File.exists?(DEPLOY_DIRECTORY)
	includes = []
	Dir["#{BIN_DIRECTORY}/*.dll"].each do |f| 
		if (File.extname(f) == '.dll')
			includes.push f
		end
	end
	sh "#{ILMERGE} /t:library /out:#{DEPLOY_FILE} #{includes.join(" ")}"
end